import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { WidgetConfig } from './widget.types';

@Injectable({ providedIn: 'root' })
export class WidgetConfigService {
  private readonly http = inject(HttpClient);
  private readonly API  = environment.apiUrl;

  getConfig(projectId: string): Observable<WidgetConfig> {
    return this.http.get<WidgetConfig>(
      `${this.API}/projects/${projectId}/widget-config`,
      { withCredentials: true }
    );
  }

  saveConfig(projectId: string, config: WidgetConfig): Observable<void> {
    return this.http.put<void>(
      `${this.API}/projects/${projectId}/widget-config`,
      config,
      { withCredentials: true }
    );
  }
}