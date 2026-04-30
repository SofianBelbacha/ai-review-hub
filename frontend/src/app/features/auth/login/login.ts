import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, Injector, inject, OnDestroy, afterNextRender, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { environment } from '../../../../environments/environment';
import { GoogleAuthService } from '../../../core/services/google-auth.service';
import { parseApiError } from '../../../core/utils/api-error.utils';


@Component({
  selector: 'app-login',
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login implements AfterViewInit, OnDestroy {
  private readonly auth   = inject(AuthService);
  private readonly router = inject(Router);
  private readonly googleAuth = inject(GoogleAuthService);
  private readonly injector = inject(Injector);



  email    = signal('');
  password = signal('');
  loading  = signal(false);
  showPass = signal(false);
  error    = signal('');
  googleLoading = signal(true);


  // ─── Lifecycle ────────────────────────────────────────────

  ngAfterViewInit(): void {
    this.googleAuth.load().then(() => {
      this.googleAuth.initialize(
        environment.googleClientId,
        (response) => this.handleGoogleResponse(response)
      );
      this.googleAuth.renderButton('google-btn', 'continue_with');
      this.googleLoading.set(false);

    });
  }

  ngOnDestroy(): void {
  }

  private loadGoogle(): Promise<void> {
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

      // Timeout de sécurité — 10s max
      setTimeout(() => {
        clearInterval(interval);
        this.error.set('Impossible de charger Google Sign-In.');
      }, 10_000);
    });
  }

  // ─── Google Identity Services ─────────────────────────────
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
        this.error.set(parseApiError(err));
      }
    });
  }
}
