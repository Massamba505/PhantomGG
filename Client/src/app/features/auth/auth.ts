import { Component } from '@angular/core';
import { RouterOutlet, RouterLink } from '@angular/router';
import { ThemeToggle } from '@/app/shared/components/ui/theme-toggle/theme-toggle';

@Component({
  selector: 'app-auth',
  standalone: true,
  imports: [RouterOutlet, RouterLink, ThemeToggle],
  templateUrl: './auth.html',
})
export class Auth {}
