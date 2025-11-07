import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ThemeToggle } from '@/app/shared/components/ui/theme-toggle/theme-toggle';

@Component({
  selector: 'app-auth',
  imports: [RouterOutlet, ThemeToggle],
  templateUrl: './auth.html',
})
export class Auth {}
