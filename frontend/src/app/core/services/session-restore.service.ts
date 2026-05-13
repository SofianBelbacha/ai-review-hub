import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpBackend } from '@angular/common/http';
import { AuthService } from './auth.service';
import { catchError, of, firstValueFrom } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class SessionRestoreService {
  private readonly auth    = inject(AuthService);
  private readonly backend = inject(HttpBackend);

  async restore(): Promise<void> {
    // Access token déjà en mémoire — pas besoin de refresh
    if (this.auth.getAccessToken()) return;

    const rawHttp = new HttpClient(this.backend);

    await firstValueFrom(
      this.auth.refreshTokens(rawHttp).pipe(
        catchError(() => of(null)) // pas de cookie valide — utilisateur non connecté
      )
    );
  }
}