import { Injectable, inject, signal } from '@angular/core';
import { HttpBackend, HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, BehaviorSubject, throwError, filter, take, switchMap, of } from 'rxjs';
import { tap, catchError } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { TokenStorageService } from './token-storage.service';

export interface AuthTokens {
  accessToken: string;
  refreshToken: string;
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

  private readonly API = environment.apiUrl;

  // ─── Signal public ────────────────────────────────────────
  isAuthenticated = signal(!!this.storage.getAccessToken());

  // ─── Gestion refresh concurrent ───────────────────────────
  private isRefreshing = false;
  private refreshSubject = new BehaviorSubject<AuthTokens | null>(null);

  // ─── Auth classique ───────────────────────────────────────
  login(email: string, password: string): Observable<AuthTokens> {
    return this.http
      .post<AuthTokens>(`${this.API}/auth/login`, { email, password })
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
        { email, password, firstName, lastName })
      .pipe(tap(tokens => this.saveTokens(tokens)));
  }

  // ─── Google OAuth ─────────────────────────────────────────
  loginWithGoogle(idToken: string): Observable<GoogleAuthResponse> {
    return this.http
      .post<GoogleAuthResponse>(`${this.API}/auth/google`, { idToken })
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

    const refreshToken = this.storage.getRefreshToken();

    if (!refreshToken) {
      this.isRefreshing = false;
      this.refreshSubject.complete();
      this.refreshSubject = new BehaviorSubject<AuthTokens | null>(null);
      this.logout();
      return throwError(() => new Error('No refresh token'));
    }

    return rawHttp
      .post<AuthTokens>(`${this.API}/auth/refresh`, { token: refreshToken })
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
          this.logout();
          return throwError(() => error);
        })
      );
  }

  // ─── Session ──────────────────────────────────────────────
  saveTokens(tokens: AuthTokens): void {
    this.storage.saveTokens(tokens);
    this.isAuthenticated.set(true);
  }

  getAccessToken(): string | null {
    return this.storage.getAccessToken();
  }

  logout(revokeOnServer = true): void {
    const refreshToken = this.storage.getRefreshToken();

    if (revokeOnServer && refreshToken) {
      // Fire and forget — on logout même si ça échoue
      this.rawHttp.post(`${this.API}/auth/revoke`, { token: refreshToken }).subscribe({error: () => {}});
    }

    this.storage.clearTokens();
    this.isAuthenticated.set(false);

    // Annule état refresh
    this.isRefreshing = false;
    this.refreshSubject.next(null);

    this.router.navigate(['/login']);
  }
}