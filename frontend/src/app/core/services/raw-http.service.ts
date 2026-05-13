import { Injectable, inject } from '@angular/core';
import { HttpBackend, HttpClient } from '@angular/common/http';

/**
 * Fournit un HttpClient singleton qui bypasse tous les intercepteurs.
 * Utilisé pour les appels auth (refresh, revoke) qui ne doivent pas
 * être interceptés par authInterceptor — évite les boucles infinies
 * et les race conditions dues à la recréation de HttpClient dans l'intercepteur.
 */
@Injectable({ providedIn: 'root' })
export class RawHttpService {
  readonly client = new HttpClient(inject(HttpBackend));
}