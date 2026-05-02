import { Injectable, NgZone, inject } from '@angular/core';
import { environment } from '../../../environments/environment';

export type GoogleButtonText =
  | 'signin_with'
  | 'signup_with'
  | 'continue_with'
  | 'signin';

@Injectable({ providedIn: 'root' })
export class GoogleAuthService {
  private readonly zone = inject(NgZone);
  private initialized  = false;  
  private activeCallback?: (response: google.accounts.id.CredentialResponse) => void;

  load(): Promise<void> {
    return new Promise(resolve => {
      if (typeof google !== 'undefined') {
        resolve();
        return;
      }

      const interval = setInterval(() => {
        if (typeof google !== 'undefined') {
          clearInterval(interval);
          resolve();
        }
      }, 100);

      setTimeout(() => clearInterval(interval), 10_000);
    });
  }

  private ensureInitialized(): void {
    if (this.initialized) return;

    google.accounts.id.initialize({
      client_id: environment.googleClientId,
      // Le callback dispatch vers le callback actif
      callback: (response) => {
        this.zone.run(() => this.activeCallback?.(response));
      },
      auto_select: false,
      cancel_on_tap_outside: true,
    });

    this.initialized = true;
  }

  // ─── Rendu du bouton avec callback spécifique ─────────────
  renderButton(
    elementId: string,
    callback: (response: google.accounts.id.CredentialResponse) => void,
    text: GoogleButtonText = 'continue_with'
  ): void {
    // Enregistre le callback actif pour ce bouton
    this.activeCallback = callback;

    this.ensureInitialized();

    const el = document.getElementById(elementId);
    if (!el) {
      console.warn(`[GoogleAuth] Element #${elementId} not found`);
      return;
    }

    google.accounts.id.renderButton(el, {
      type: 'standard',
      shape: 'rectangular',
      theme: 'outline',
      text,
      size: 'large',
      logo_alignment: 'left',
    });
  }
  // ─── Reset au logout ──────────────────────────────────────
  reset(): void {
    this.initialized = false;
    this.activeCallback = undefined;

    if (typeof google !== 'undefined') {
      google.accounts.id.cancel();
    }
  }
}
