import { Routes } from '@angular/router';
import { DashboardShell } from './features/dashboard/shell/dashboard-shell';

export const routes: Routes = [
    {
        path: '',
        loadComponent: () =>
        import('./features/landing/landing').then(m => m.Landing)
    },
    {
        path: 'login',
        loadComponent: () => import('./features/auth/login/login').then(m => m.Login)
    },
    {
        path: 'register',
        loadComponent: () => import('./features/auth/register/register').then(m => m.Register)
    },
    {
        path: 'dashboard',
        component: DashboardShell,
        children: [
            { path: '',         loadComponent: () => import('./features/dashboard/home/dashboard-home').then(m => m.DashboardHome) },
            { path: 'projects', loadComponent: () => import('./features/dashboard/projects/projects').then(m => m.Projects) },
            { path: 'feedbacks',loadComponent: () => import('./features/dashboard/feedbacks/feedbacks').then(m => m.Feedbacks) },
        ]
    }

];
