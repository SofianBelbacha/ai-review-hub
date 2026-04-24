import { Component, computed, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Footer } from '../../shared/components/footer/footer';
import { Navbar } from '../../shared/components/navbar/navbar';


export interface ProblemItem {
  id: string;
  icon: string;
  title: string;
  body: string;
  quote: string;
  accent: 'neutral' | 'red' | 'amber';
}

export interface SolutionStep {
  id: string;
  number: string;
  title: string;
  description: string;
  icon: string;
  details: string[];
}

export interface FeatureCard {
  id: string;
  title: string;
  description: string;
  image: string;
  color: string;
}

export interface VideoFeature {
  id: string;
  title: string;
  description: string;
  videoSrc?: string;   
  posterSrc?: string;  
  accent: string;   
}

export interface PricingPlan {
  id: string;
  name: string;
  price: number | null;
  priceLabel?: string;
  description: string;
  cta: string;
  ctaPath: string;
  featured: boolean;
  features: string[];
}

export interface FaqItem {
  id: string;
  question: string;
  answer: string;
}


@Component({
  selector: 'app-landing',
  imports: [CommonModule, RouterLink, Navbar, Footer],
  templateUrl: './landing.html',
  styleUrl: './landing.scss',
})
export class Landing {

  // --- Hero tabs ---
  activeTab = signal<string>('Project');
 
  tabs = ['Project', 'Task', 'Message', 'Invoice', 'Clients', 'Timer'];
 
  setActiveTab(tab: string): void {
    this.activeTab.set(tab);
  }

    // ── Problem — Accordéon ───────────────────────────────
  activeProblем = signal<string>('scattered');
 
  problemItems: ProblemItem[] = [
    {
      id: 'scattered',
      icon: 'chat',
      title: 'Les retours arrivent de partout',
      body: 'Email, Slack, Notion, WhatsApp, post-its… Chaque client utilise un canal différent. Résultat : vous passez votre temps à consolider avant même de commencer à corriger.',
      quote: '"Je retrouve encore des retours dans ma boîte mail d\'il y a 3 semaines que j\'avais complètement oubliés."',
      accent: 'neutral',
    },
    {
      id: 'priority',
      icon: 'alert',
      title: 'Rien n\'est priorisé',
      body: 'Un bug bloquant côtoie une demande cosmétique sans distinction. Sans hiérarchisation, l\'équipe traite ce qui arrive en premier — pas ce qui compte le plus.',
      quote: '"On a passé une journée à corriger la couleur d\'un bouton pendant qu\'un formulaire planté bloquait les ventes."',
      accent: 'red',
    },
    {
      id: 'time',
      icon: 'clock',
      title: 'Le tri manuel prend des heures',
      body: 'Lire, catégoriser, résumer, répondre, reporter dans un fichier… La phase de recettage dure 2x plus longtemps que nécessaire à cause de cette friction invisible.',
      quote: '"Je passe facilement 3h à préparer la réunion de suivi client. C\'est du temps pas facturé."',
      accent: 'amber',
    },
    {
      id: 'context',
      icon: 'users',
      title: 'Le contexte se perd',
      body: 'Un développeur reçoit un ticket sans savoir si c\'est urgent, à quel projet ça appartient, ni ce que le client voulait vraiment dire. Il doit relancer. Le client s\'impatiente.',
      quote: '"On a corrigé le mauvais bug parce que le retour client était flou et on a interprété trop vite."',
      accent: 'neutral',
    },
    {
      id: 'visibility',
      icon: 'chart',
      title: 'Aucune visibilité sur l\'avancement',
      body: 'Le client ne sait pas où en est son retour. L\'équipe non plus, clairement. Il faut un échange pour avoir un statut — ce qui génère encore plus de retours.',
      quote: '"Mon client m\'appelle chaque semaine pour savoir où en est la correction qu\'il a demandée il y a 10 jours."',
      accent: 'neutral',
    },
    {
      id: 'tools',
      icon: 'grid',
      title: 'Trop d\'outils, pas de solution',
      body: 'Trello, Notion, Jira, email, Google Sheets… Chaque équipe bricole son propre système. Aucun n\'est pensé pour la relation client et la gestion des feedbacks en recettage.',
      quote: '"On utilise 5 outils différents pour gérer un seul projet. C\'est un problème de coordination permanent."',
      accent: 'neutral',
    },
  ];
 
  activeProblem = computed(() =>
    this.problemItems.find(p => p.id === this.activeProblем()) ?? this.problemItems[0]
  );
 
  setActiveProblem(id: string): void {
    this.activeProblем.set(id);
  }

  // Solution section 
  solutionSteps: SolutionStep[] = [
    {
      id: 's1',
      number: '01',
      title: 'Collectez sans friction',
      description: 'Vos clients soumettent leurs retours via un widget intégré ou un lien public. Aucun compte requis. Aucune friction.',
      icon: 'widget',
      details: [
        'Widget JS en 2 lignes de code',
        'Lien public partageable',
        'Formulaire simple, sans inscription',
      ],
    },
    {
      id: 's2',
      number: '02',
      title: 'L\'IA trie et priorise',
      description: 'Chaque retour est automatiquement catégorisé (bug, feature, question), résumé en une phrase et scoré selon le sentiment détecté.',
      icon: 'ai',
      details: [
        'Catégorisation automatique',
        'Résumé IA en une phrase',
        'Score de priorité par sentiment',
      ],
    },
    {
      id: 's3',
      number: '03',
      title: 'Agissez sur ce qui compte',
      description: 'Votre kanban affiche les retours priorisés. Vous traitez en premier ce qui bloque vraiment, pas ce qui est arrivé en dernier.',
      icon: 'kanban',
      details: [
        'Kanban visuel par projet',
        'Filtres par catégorie et priorité',
        'Graphique de tendances 30 jours',
      ],
    },
  ];

    // --- Features ---
  featuresRow1: FeatureCard[] = [
    {
      id: 'ai-analysis',
      title: 'Analyse IA en temps réel',
      description: 'Chaque feedback est instantanément catégorisé et priorisé par l\'IA. Plus besoin de trier manuellement.',
      image: 'image/project_manager.png',
      color: '#EEF2FF',
    },
    {
      id: 'kanban',
      title: 'Kanban visuel',
      description: 'Visualisez l\'état de tous vos retours en un coup d\'œil. Déplacez les cartes de "À traiter" à "Résolu".',
      image: 'image/collaboration.png',
      color: '#F0FDF4',
    },
  ];
 
  featuresRow2: FeatureCard[] = [
    {
      id: 'widget',
      title: 'Widget intégrable',
      description: 'Un simple snippet JS à coller sur le site de votre client. Aucun compte requis pour soumettre un retour.',
      image: 'image/track.png',
      color: '#FFF7ED',
    },
    {
      id: 'projects',
      title: 'Multi-projets',
      description: 'Gérez plusieurs clients et projets depuis un seul tableau de bord centralisé.',
      image: 'image/integration.png',
      color: '#F0F9FF',
    },
    {
      id: 'trends',
      title: 'Tendances & insights',
      description: 'Un graphique des 30 derniers jours pour détecter les pics de feedback et anticiper les problèmes.',
      image: 'image/trends.png',
      color: '#FDF4FF',
    },
  ];

  // -----------------------------------------------
  // Video Features — section 3 colonnes avec média
  // -----------------------------------------------
  videoFeatures: VideoFeature[] = [
    {
      id: 'widget-embed',
      title: 'Intégrez le widget en 2 minutes',
      description: 'Copiez un snippet JS, collez-le sur le site de votre client. Vos retours arrivent instantanément.',
      accent: '#EEF2FF',
      videoSrc: 'https://bytescale.mobbin.com/FW25bBB/video/mobbin.com/prod/assets/file.mp4?enc=1.BQnbdJK6.NGEe6x9i9aOYzTwJ.DK1BKCNO6aQVJATLtV2MuCmrTRrmN6IeXY1MHD5g_TSXfti0oVE0Uxn-SsfdscCUw3P6wmNBksSzSPgNkJYwBlWgIft1ekFhexHCiQB-fTq_rigQuRQCxgKSha-LXmhDrXgTH8mxVkmI4wmRWIP8_R9s57g2zCOvho7ALvbdcv9bYFQBbgCa_J4vk6K40y-j_gJFJPdIiTNje05WZHaEj34Tkrhh1oiVmVhBtR2Yzx5Lgbx9TVd91tUKxT6oevSRYWaiTwnlLbUk77rdjnDqt9ojWbM2v7tIM3-a5G-JNpu-qWT_yjMurZovYbhnkBOSWQ',
    },
    {
      id: 'ai-triage',
      title: 'L\'IA trie à votre place',
      description: 'Catégorie, résumé, score de priorité — chaque feedback est enrichi en moins de 3 secondes.',
      accent: '#F0FDF4',
      videoSrc: 'https://bytescale.mobbin.com/FW25bBB/video/mobbin.com/prod/assets/file.mp4?enc=1.BQnbdJK6.9kH0UglFp7TtiXRV.1UVODMWIhz8S5i1qazUyqCAqQKAfI_-KgUG0dQNbPscKw9lLySH3Gow1rhPyqmXnC80XXuPYEUQfS8BRcdgdDkDX5CKIcCkTO46i1gxtMk7PzKo6w_x7wUICj4axruWL0N0UFYVHlYVU0CdSK9Ks5eAEBqbACpZ2DtM5H8ifavKZSZ-zb6Slnvy7a33lYzWiGb71hzbyJQdIXmFKLznT-BymOXgqRGkTbSvN5slZlFS1ZoXMxKJMgmk6uu1UBODz-sE2OHDKc6oL4rBVhkk7yIz2m7_wyGsGmkMlDMmQfJMJyLrXoNjcQfpggcjAMGDjAKteXZ48GQ',
    },
    {
      id: 'kanban-flow',
      title: 'Du feedback au resolved',
      description: 'Glissez les cartes de "À traiter" à "Résolu". Votre workflow, visualisé en temps réel.',
      accent: '#FFF7ED',
      videoSrc: 'https://bytescale.mobbin.com/FW25bBB/video/mobbin.com/prod/assets/file.mp4?enc=1.BQnbdJK6.fzCCFro2Ta7vlIT3.8j-Yv5SpGXhrOEjiHLg2es1HzA05kSarikmKP2VM7Hu4AlUdpwBVz5E3RzoP5F272sFHC5e_dEit9W7R6EAMre_8XTGFsZBkWBabNoKIqOpdwawNYB1rig9CAyyw7VZdhQLhRRyjd3CxljNVb67mUUCiRQk1NMuDM-bHL4JR46jkG6y8r-pSEjcNW2QleZn7f-nTy1qGYZpO0qF2P97js89b-3js2BdIZLd7jbJ0wRW0yn-gf_dZaxGFm6U2OtXcR3Q6zMXjOCgxxV4mCB1N70uTwLKE1WYfrFhtqzLyfs_LZcU08-ppMR_Lu7Q',
    },
  ];


  // -----------------------------------------------
  // Pricing
  // -----------------------------------------------
  plans: PricingPlan[] = [
    {
      id: 'free',
      name: 'Free',
      price: 0,
      description: 'Pour tester la valeur du produit sans engagement.',
      cta: 'Commencer gratuitement',
      ctaPath: '/register',
      featured: false,
      features: [
        '1 projet actif',
        '50 feedbacks / mois',
        'Analyse IA (catégorie + résumé)',
        'Tableau kanban',
        'Widget intégrable',
        'Support par email',
      ],
    },
    {
      id: 'pro',
      name: 'Pro',
      price: 9,
      description: 'Pour les freelances et indépendants qui gèrent plusieurs clients.',
      cta: 'Passer au Pro',
      ctaPath: '/register?plan=pro',
      featured: true,
      features: [
        '10 projets actifs',
        'Feedbacks illimités',
        'Analyse IA complète + score de priorité',
        'Tableau kanban + filtres avancés',
        'Widget intégrable personnalisable',
        'Graphique de tendances 30 jours',
        'Export CSV',
        'Support prioritaire',
      ],
    },
    {
      id: 'team',
      name: 'Team',
      price: 29,
      description: 'Pour les agences qui travaillent en équipe sur les mêmes projets.',
      cta: 'Contacter l\'équipe',
      ctaPath: '/contact',
      featured: false,
      features: [
        'Projets illimités',
        'Feedbacks illimités',
        'Tout le plan Pro',
        'Membres d\'équipe illimités',
        'Gestion des rôles & permissions',
        'Tableau de bord partagé',
        'Intégrations (Slack, Notion…)',
        'Support dédié + onboarding',
      ],
    },
  ];


  // -----------------------------------------------
  // FAQ — état d'ouverture géré avec un signal
  // -----------------------------------------------
  openFaqId = signal<string | null>(null);
 
  toggleFaq(id: string): void {
    this.openFaqId.update(current => current === id ? null : id);
  }
 
  faqs: FaqItem[] = [
    {
      id: 'faq-1',
      question: 'Comment fonctionne l\'analyse IA des feedbacks ?',
      answer: 'Dès qu\'un retour est soumis via le widget ou le lien public, notre backend l\'envoie à l\'API OpenAI. En quelques secondes, l\'IA retourne trois informations : la catégorie du retour (bug, feature request ou question), un résumé en une phrase claire, et un score de priorité calculé à partir de l\'analyse du sentiment. Ces données enrichissent automatiquement votre kanban.',
    },
    {
      id: 'faq-2',
      question: 'Mes clients ont-ils besoin de créer un compte pour soumettre un retour ?',
      answer: 'Non, c\'est l\'un de nos partis pris forts. Votre client accède à un lien public ou interagit avec un widget JavaScript intégré directement sur son site. Il remplit un simple formulaire et soumet son retour — sans inscription, sans mot de passe, sans friction.',
    },
    {
      id: 'faq-3',
      question: 'Quelle est la différence entre le plan Free et le plan Pro ?',
      answer: 'Le plan Free autorise 1 projet actif et jusqu\'à 50 feedbacks par mois, ce qui est suffisant pour tester la plateforme avec un premier client. Le plan Pro (9 €/mois) débloque 10 projets, les feedbacks illimités, les filtres avancés, l\'export CSV et le graphique de tendances. Le plan Team ajoute la gestion multi-membres et les intégrations tierces.',
    },
    {
      id: 'faq-4',
      question: 'Puis-je annuler mon abonnement à tout moment ?',
      answer: 'Oui, sans engagement ni frais cachés. Vous pouvez annuler depuis votre espace compte en un clic. Votre accès Pro reste actif jusqu\'à la fin de la période déjà facturée, puis bascule automatiquement sur le plan Free.',
    },
    {
      id: 'faq-5',
      question: 'Le widget est-il compatible avec tous les types de sites ?',
      answer: 'Oui. Le widget est un simple snippet JavaScript universel — il fonctionne sur n\'importe quel site web, qu\'il soit construit avec WordPress, Webflow, un framework React ou Vue, ou même du HTML statique. L\'intégration prend moins de deux minutes.',
    },
    {
      id: 'faq-6',
      question: 'Mes données sont-elles sécurisées ?',
      answer: 'Les données sont hébergées sur une infrastructure cloud certifiée (Railway / Supabase) avec chiffrement en transit (TLS) et au repos. L\'authentification utilise un système JWT avec refresh tokens. Nous ne revendons jamais vos données ni celles de vos clients à des tiers.',
    },
  ];


}
