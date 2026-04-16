import { Routes } from '@angular/router';
import { Landing } from './features/landing/landing';

export const routes: Routes = [
    {
        path: '',
        loadComponent: () =>
        import('./features/landing/landing').then(m => m.Landing)
    }

];
