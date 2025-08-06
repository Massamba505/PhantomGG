import { Component, signal } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.html',
})
export class Login {
  constructor(private router: Router) {}
  showPassword = signal<boolean>(false)


  onSubmit() {
    console.log('Login submitted');
  }

  togglePassword(){
    this.showPassword.update(x=> !x);
  }
}
