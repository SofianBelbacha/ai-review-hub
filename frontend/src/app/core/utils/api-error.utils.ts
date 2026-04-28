import { HttpErrorResponse } from '@angular/common/http';

export function parseApiError(err: HttpErrorResponse): string {
  switch (err.status) {
    case 400:
      if (err.error?.errors) {
        const messages = Object.values(err.error.errors).flat() as string[];
        return messages[0] ?? 'Données invalides.';
      }
      return 'Requête invalide.';
    case 401:
      return 'Email ou mot de passe incorrect.';
    case 409:
      return 'Cette adresse email est déjà utilisée.';
    case 429:
      return 'Trop de tentatives. Réessayez dans quelques minutes.';
    case 0:
    case 503:
      return 'Service indisponible. Vérifiez votre connexion.';
    default:
      return 'Une erreur est survenue. Veuillez réessayer.';
  }
}