import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, inject, NgZone, OnDestroy, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-login',
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login implements AfterViewInit, OnDestroy {
  private readonly auth   = inject(AuthService);
  private readonly router = inject(Router);
  private readonly zone   = inject(NgZone);

  email    = signal('');
  password = signal('');
  loading  = signal(false);
  showPass = signal(false);
  error    = signal('');

  // ─── Lifecycle ────────────────────────────────────────────
  ngAfterViewInit(): void {
    this.initGoogleButton();
  }

  ngOnDestroy(): void {
    // Nettoyage si nécessaire
    google?.accounts?.id?.cancel();
  }

  // ─── Google Identity Services ─────────────────────────────
  private initGoogleButton(): void {
    if (typeof google === 'undefined') {
      // Script pas encore chargé — réessaie après 500ms
      console.log('Google SDK pas encore chargé, retry...');
      setTimeout(() => this.initGoogleButton(), 500);
      return;
    }

    google.accounts.id.initialize({
      client_id: environment.googleClientId,
      callback: (response) => {
        // NgZone — Google appelle le callback hors de la zone Angular
        this.zone.run(() => this.handleGoogleResponse(response));
      },
      auto_select: false,
      cancel_on_tap_outside: true,
    });

    google.accounts.id.renderButton(
      document.getElementById('google-btn')!,
      {
        type: 'standard',
        shape: 'rectangular',
        theme: 'outline',
        text: 'continue_with',
        size: 'large',
        logo_alignment: 'left',
        width: '100%',
      }
    );
  }

  private handleGoogleResponse(response: google.accounts.id.CredentialResponse): void {
    if (!response.credential) {
      this.error.set('Échec de la connexion Google.');
      return;
    }

    this.loading.set(true);
    this.error.set('');

    this.auth.loginWithGoogle(response.credential).subscribe({
      next: (result) => {
        this.auth.saveTokens(result);
        this.router.navigate([result.isNewUser ? '/onboarding' : '/dashboard']);
      },
      error: () => {
        this.loading.set(false);
        this.error.set('Échec de la connexion Google. Réessayez.');
      }
    });
  }

  // ─── Formulaire classique ─────────────────────────────────
  togglePassword(): void {
    this.showPass.update(v => !v);
  }

  onSubmit(): void {
    this.error.set('');

    if (!this.email() || !this.password()) {
      this.error.set('Veuillez remplir tous les champs.');
      return;
    }

    this.loading.set(true);

    this.auth.login(this.email(), this.password()).subscribe({
      next: (tokens) => {
        this.auth.saveTokens(tokens);
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        this.loading.set(false);
        this.error.set(
          err.status === 401
            ? 'Email ou mot de passe incorrect.'
            : 'Une erreur est survenue. Réessayez.'
        );
      }
    });
  }
}
