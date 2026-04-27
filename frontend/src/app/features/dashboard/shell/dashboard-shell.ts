import { Component, HostListener, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
 
interface NavItem {
  label: string;
  path: string;
  icon: string;
  badge?: number;
}

@Component({
  selector: 'app-dashboard-shell',
  imports: [CommonModule, RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './dashboard-shell.html',
  styleUrl: './dashboard-shell.scss',
})
export class DashboardShell {
  
  sidebarCollapsed = signal(false);
  mobileMenuOpen   = signal(false);
 
  // Projet actif simulé
  currentProject = signal({ name: 'Refonte e-commerce', plan: 'Pro' });
 
  navItems: NavItem[] = [
    { label: 'Vue d\'ensemble', path: '/dashboard', icon: 'home'},
    { label: 'Projets', path: '/dashboard/projects', icon: 'folder', badge: 3},
    { label: 'Feedbacks', path: '/dashboard/feedbacks', icon: 'messages', badge: 12},
    { label: 'Tendances', path: '/dashboard/trends', icon: 'chart'},
    { label: 'Widget', path: '/dashboard/widget', icon: 'code'},
  ];
 
  bottomNavItems: NavItem[] = [
    { label: 'Paramètres', path: '/dashboard/settings', icon: 'settings'},
    { label: 'Aide', path: '/dashboard/help', icon: 'help'},
  ];
 
  toggleSidebar(): void {
    this.sidebarCollapsed.update(v => !v);
  }
 
  toggleMobileMenu(): void {
    this.mobileMenuOpen.update(v => !v);
    document.body.style.overflow = this.mobileMenuOpen() ? 'hidden' : '';
  }
 
  closeMobileMenu(): void {
    this.mobileMenuOpen.set(false);
    document.body.style.overflow = '';
  }
 
  @HostListener('window:keydown.escape')
  onEscape(): void {
    this.closeMobileMenu();
  }

}
