import { Component, signal } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-signup',
  imports: [],
  templateUrl: './signup.html'
})
export class Signup {
  constructor(private router: Router) {}
  showPassword = signal<boolean>(false);
  showPassword2 = signal<boolean>(false);

  onSubmit() {
    console.log('Signup submitted');
  }

  togglePassword(){
    this.showPassword.update(x=> !x);
  }

  togglePassword2(){
    this.showPassword2.update(x=> !x);
  }
}
