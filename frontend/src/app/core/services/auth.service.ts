import { Injectable, inject, signal } from '@angular/core';
import { HttpBackend, HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, BehaviorSubject, throwError, filter, take, switchMap, of } from 'rxjs';
import { tap, catchError, finalize } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { TokenStorageService } from './token-storage.service';
import { UserService } from './user.service';


export interface AuthTokens {
  accessToken: string;
}

export interface GoogleAuthResponse extends AuthTokens {
  isNewUser: boolean;
}

@Injectable({ providedIn: 'root' })

export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly rawHttp = new HttpClient(inject(HttpBackend));
  private readonly router  = inject(Router);
  private readonly storage = inject(TokenStorageService);
  private readonly userService = inject(UserService);
  private readonly API = environment.apiUrl;

  // ─── Signal public ────────────────────────────────────────
  isAuthenticated = signal(!!this.storage.getAccessToken());

  // ─── Gestion refresh concurrent ───────────────────────────
  private isRefreshing = false;
  private refreshSubject = new BehaviorSubject<AuthTokens | null>(null);

  // ─── Auth classique ───────────────────────────────────────
  login(email: string, password: string): Observable<AuthTokens> {
    return this.http
      .post<AuthTokens>(`${this.API}/auth/login`, { email, password }, { withCredentials: true })
      .pipe(tap(tokens => this.saveTokens(tokens)));
  }

  register(
    email: string,
    password: string,
    firstName: string,
    lastName: string
  ): Observable<AuthTokens> {
    return this.http
      .post<AuthTokens>(`${this.API}/auth/register`,
        { email, password, firstName, lastName }, { withCredentials: true })
      .pipe(tap(tokens => this.saveTokens(tokens)));
  }

  // ─── Google OAuth ─────────────────────────────────────────
  loginWithGoogle(idToken: string): Observable<GoogleAuthResponse> {
    return this.http
      .post<GoogleAuthResponse>(`${this.API}/auth/google`, { idToken }, { withCredentials: true })
      .pipe(tap(result => this.saveTokens(result)));
  }

  // ─── Refresh avec queue ───────────────────────────────────
  refreshTokens(rawHttp: HttpClient): Observable<AuthTokens> {
    // Un refresh est déjà en cours — on attend le résultat
    if (this.isRefreshing) {
      return this.refreshSubject.pipe(
        filter(token => token !== null),
        take(1),
        switchMap(tokens => of(tokens!))
      );
    }

    this.isRefreshing = true;
    this.refreshSubject.next(null); // bloque les autres

    return rawHttp
      .post<AuthTokens>(`${this.API}/auth/refresh`, {}, { withCredentials: true })
      .pipe(
        tap(tokens => {
          this.saveTokens(tokens);
          this.refreshSubject.next(tokens); // débloque la queue
          this.isRefreshing = false;
        }),
        catchError(error => {
          this.isRefreshing = false;
          // Termine proprement le subject courant
          this.refreshSubject.complete();
          // Recrée un nouveau subject sain pour les futurs refresh
          this.refreshSubject = new BehaviorSubject<AuthTokens | null>(null);
          this.logout(false);
          return throwError(() => error);
        })
      );
  }

  // ─── Session ──────────────────────────────────────────────
  saveTokens(tokens: AuthTokens): void {
    this.storage.saveAccessToken(tokens.accessToken);
    this.userService.refresh(); // ← décode le JWT et met à jour le profil
    this.isAuthenticated.set(true);
  }

  getAccessToken(): string | null {
    return this.storage.getAccessToken();
  }

  logout(revokeOnServer = true): void {
    const completeLogout = () => {
      this.storage.clearAll();
      this.userService.clear(); // ← efface le profil
      this.isAuthenticated.set(false);
      this.isRefreshing = false;
      this.refreshSubject.next(null);
      this.router.navigate(['/login']);
    };

    if (revokeOnServer) {
      // Fire and forget — cookie envoyé automatiquement
      this.rawHttp
        .post(
          `${this.API}/auth/revoke`,
          { revokeAll: false },
          { withCredentials: true })
        .pipe(
          // Logout local dans tous les cas — succès ou échec du revoke
          finalize((completeLogout)))
        .subscribe({ error: () => {} });
    }
  }
}