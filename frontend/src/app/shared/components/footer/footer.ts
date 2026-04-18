import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

interface FooterLink {
  label: string;
  path?: string;   // routerLink interne
  href?: string;   // lien externe
}

interface FooterColumn {
  title: string;
  links: FooterLink[];
}

@Component({
  selector: 'app-footer',
  imports: [CommonModule, RouterLink],
  templateUrl: './footer.html',
  styleUrl: './footer.scss',
})
export class Footer {

  currentYear = new Date().getFullYear();

  socials = [
    { id: 'linkedin', label: 'LinkedIn', href: 'https://linkedin.com', icon: 'linkedin' },
    { id: 'twitter', label: 'Twitter/X', href: 'https://x.com', icon: 'twitter' },
    { id: 'github', label: 'GitHub', href: 'https://github.com', icon: 'github' },
    { id: 'youtube', label: 'YouTube', href: 'https://youtube.com', icon: 'youtube' },
  ];

  columns: FooterColumn[] = [
    {
      title: 'Produit',
      links: [
        { label: 'Feedbacks', path: '/feedbacks' },
        { label: 'Projets', path: '/projects' },
        { label: 'Tableau de bord', path: '/dashboard' },
        { label: 'Widget client', path: '/widget' },
        { label: 'Intégrations', path: '/integrations' },
      ],
    },
    {
      title: 'Solutions',
      links: [
        { label: 'Agences web', path: '/solutions/agencies' },
        { label: 'Freelances', path: '/solutions/freelancers' },
        { label: 'Équipes IT', path: '/solutions/it-teams' },
        { label: 'PME', path: '/solutions/smb' },
      ],
    },
    {
      title: 'Ressources',
      links: [
        { label: 'Blog', path: '/blog' },
        { label: 'Documentation', path: '/docs' },
        { label: 'API', href: 'https://api.ai-review-hub.app' },
        { label: 'Roadmap', path: '/roadmap' },
        { label: 'Centre d\'aide', path: '/help' },
      ],
    },
    {
      title: 'Entreprise',
      links: [
        { label: 'Tarifs', path: '/pricing' },
        { label: 'Nous contacter', path: '/contact' },
        { label: 'Conditions d\'utilisation', path: '/terms' },
        { label: 'Politique de confidentialité', path: '/privacy' },
      ],
    },
  ];

}
