import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Navbar } from './shared/components/navbar/navbar';
import { HttpBackend, HttpClient } from '@angular/common/http';
import { AuthService } from './core/services/auth.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Navbar],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App implements OnInit {
  private readonly auth     = inject(AuthService);
  private readonly backend  = inject(HttpBackend);

  ngOnInit(): void {
    this.tryRestoreSession();
  }

  private tryRestoreSession(): void {
    // Si pas d'access token en mémoire — tente un refresh silencieux
    if (!this.auth.getAccessToken()) {
      const rawHttp = new HttpClient(this.backend);

      this.auth.refreshTokens(rawHttp).subscribe({
        next: () => {
          // Session restaurée silencieusement
        },
        error: () => {
          // Pas de cookie valide — utilisateur non connecté, rien à faire
        }
      });
    }
  }
}
