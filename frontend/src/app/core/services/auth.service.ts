// src/app/core/services/auth.service.ts
import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';

export interface AuthTokens {
  accessToken: string;
  refreshToken: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http   = inject(HttpClient);
  private readonly router = inject(Router);

  private readonly API = environment.apiUrl;

  isAuthenticated = signal(!!this.getAccessToken());

  // ─── Classique ──────────────────────────────────────────
  login(email: string, password: string) {
    return this.http.post<AuthTokens>(`${this.API}/auth/login`, { email, password });
  }

  register(email: string, password: string, firstName: string, lastName: string) {
    return this.http.post<AuthTokens>(`${this.API}/auth/register`,
      { email, password, firstName, lastName });
  }

  // ─── Google OAuth ────────────────────────────────────────
  loginWithGoogle(idToken: string) {
    return this.http.post<AuthTokens & { isNewUser: boolean }>(
      `${this.API}/auth/google`, { idToken });
  }

  // ─── Session ─────────────────────────────────────────────
  saveTokens(tokens: AuthTokens): void {
    localStorage.setItem('access_token',  tokens.accessToken);
    localStorage.setItem('refresh_token', tokens.refreshToken);
    this.isAuthenticated.set(true);
  }

  getAccessToken(): string | null {
    return localStorage.getItem('access_token');
  }

  logout(): void {
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
    this.isAuthenticated.set(false);
    this.router.navigate(['/login']);
  }
}