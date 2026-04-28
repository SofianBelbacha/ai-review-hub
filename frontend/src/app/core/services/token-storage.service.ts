import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class TokenStorageService {
  // Access token en mémoire — perdu au refresh de page (normal)
  private accessToken: string | null = null;

  // ─── Access token (mémoire) ───────────────────────────────
  saveAccessToken(token: string): void {
    this.accessToken = token;
  }

  getAccessToken(): string | null {
    return this.accessToken;
  }

  clearAccessToken(): void {
    this.accessToken = null;
  }

  // ─── Refresh token (httpOnly cookie géré par le navigateur) ──
  // Le refresh token est envoyé automatiquement par le navigateur
  // via withCredentials — on ne le lit jamais côté JS

  // ─── JWT decode pour expiration proactive ─────────────────
  getTokenExpiration(): Date | null {
    if (!this.accessToken) return null;

    try {
      const payload = JSON.parse(atob(this.accessToken.split('.')[1]));
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

  // ─── Clear complet ─────────────────────────────────────────
  clearAll(): void {
    this.accessToken = null;
    // Le cookie refresh_token est supprimé par le backend via /auth/revoke
  }
}