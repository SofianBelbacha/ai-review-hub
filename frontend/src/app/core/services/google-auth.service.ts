import { Injectable, NgZone, inject } from '@angular/core';

export type GoogleButtonText =
  | 'signin_with'
  | 'signup_with'
  | 'continue_with'
  | 'signin';

@Injectable({ providedIn: 'root' })
export class GoogleAuthService {
  private readonly zone = inject(NgZone);

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

  initialize(
    clientId: string,
    callback: (response: google.accounts.id.CredentialResponse) => void
  ): void {
    google.accounts.id.initialize({
      client_id: clientId,
      callback: (response) => this.zone.run(() => callback(response)),
      auto_select: false,
      cancel_on_tap_outside: true,
    });
  }

  renderButton(elementId: string, text: GoogleButtonText = 'continue_with'): void {
    const el = document.getElementById(elementId);
    if (!el) return;

    google.accounts.id.renderButton(el, {
      type: 'standard',
      shape: 'rectangular',
      theme: 'outline',
      text,
      size: 'large',
      logo_alignment: 'left',
      width: '100%',
    });
  }
}