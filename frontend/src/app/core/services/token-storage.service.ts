import { Injectable } from '@angular/core';
import { AuthTokens } from './auth.service';

@Injectable({ providedIn: 'root' })
export class TokenStorageService {
  // TODO prod : access token en mémoire + refresh token en httpOnly cookie
  private readonly ACCESS_KEY  = 'access_token';
  private readonly REFRESH_KEY = 'refresh_token';

  saveTokens(tokens: AuthTokens): void {
    localStorage.setItem(this.ACCESS_KEY,  tokens.accessToken);
    localStorage.setItem(this.REFRESH_KEY, tokens.refreshToken);
  }

  getAccessToken(): string | null {
    return localStorage.getItem(this.ACCESS_KEY);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(this.REFRESH_KEY);
  }

  clearTokens(): void {
    localStorage.removeItem(this.ACCESS_KEY);
    localStorage.removeItem(this.REFRESH_KEY);
  }

  // ─── Décodage JWT pour expiration proactive ────────────────
  getTokenExpiration(): Date | null {
    const token = this.getAccessToken();
    if (!token) return null;

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload.exp ? new Date(payload.exp * 1000) : null;
    } catch {
      return null;
    }
  }

  isTokenExpiringSoon(thresholdSeconds = 60): boolean {
    const exp = this.getTokenExpiration();
    if (!exp) return false;
    return (exp.getTime() - Date.now()) < thresholdSeconds * 1000;
  }
}