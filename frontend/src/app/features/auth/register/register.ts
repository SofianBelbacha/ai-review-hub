import { CommonModule } from '@angular/common';
import { Component, computed, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-register',
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './register.html',
  styleUrl: './register.scss',
})
export class Register {
    // Champs
  firstName = signal('');
  lastName  = signal('');
  email     = signal('');
  password  = signal('');
 
  // UI state
  loading   = signal(false);
  showPass  = signal(false);
  error     = signal('');
  step      = signal<1 | 2>(1); // étape 1 = infos perso, étape 2 = email + mdp
 
  // Validation mot de passe
  passwordStrength = computed(() => {
    const p = this.password();
    if (!p) return 0;
    let score = 0;
    if (p.length >= 8) score++;
    if (/[A-Z]/.test(p)) score++;
    if (/[0-9]/.test(p)) score++;
    if (/[^A-Za-z0-9]/.test(p)) score++;
    return score; // 0-4
  });
 
  passwordStrengthLabel = computed(() => {
    const labels = ['', 'Faible', 'Moyen', 'Bon', 'Fort'];
    return labels[this.passwordStrength()] ?? '';
  });
 
  passwordStrengthClass = computed(() => {
    const classes = ['', 'weak', 'fair', 'good', 'strong'];
    return classes[this.passwordStrength()] ?? '';
  });
 
  togglePassword(): void {
    this.showPass.update(v => !v);
  }
 
  nextStep(): void {
    this.error.set('');
    if (!this.firstName().trim() || !this.lastName().trim()) {
      this.error.set('Veuillez renseigner votre prénom et votre nom.');
      return;
    }
    this.step.set(2);
  }
 
  prevStep(): void {
    this.error.set('');
    this.step.set(1);
  }
 
  onSubmit(): void {
    this.error.set('');
 
    if (!this.email() || !this.password()) {
      this.error.set('Veuillez remplir tous les champs.');
      return;
    }
 
    if (this.passwordStrength() < 2) {
      this.error.set('Votre mot de passe est trop faible.');
      return;
    }
 
    this.loading.set(true);
    // emplacement pour le branchement du AuthService
    setTimeout(() => this.loading.set(false), 1500);
  }

}
