import { LoginRequest } from '@/app/shared/models/Authentication';
import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-login',
  imports: [FormsModule, RouterLink],
  templateUrl: './login.html',
})
export class Login {
  constructor() {}
  formData = signal<LoginRequest>({ email: '', password: '' });
  showPassword = signal<boolean>(false);

  onSubmit(event: Event) {
    event.preventDefault();
    console.log(this.formData());
  }

  togglePassword() {
    this.showPassword.update((x) => !x);
  }
}
