import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-login',
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login {
  email    = signal('');
  password = signal('');
  loading  = signal(false);
  showPass = signal(false);
  error    = signal('');
 
  togglePassword(): void {
    this.showPass.update(v => !v);
  }
 
  onSubmit(): void {
    this.error.set('');
 
    if (!this.email() || !this.password()) {
      this.error.set('Veuillez remplir tous les champs.');
      return;
    }
 
    this.loading.set(true);
 
    // emplacement pour le branchement du AuthService
    // this.authService.login(this.email(), this.password()).subscribe(...)
    setTimeout(() => this.loading.set(false), 1500); // simulation
  }

}
