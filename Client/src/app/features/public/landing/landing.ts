import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ThemeToggle } from '@/app/shared/components/ui/theme-toggle/theme-toggle';
import { AuthStateService } from '@/app/store/AuthStateService';

@Component({
  selector: 'app-landing',
  imports: [RouterLink, ThemeToggle],
  templateUrl: './landing.html',
  styleUrl: './landing.css',
})
export class Landing {
  private readonly authState = inject(AuthStateService);
  isAuthenticated = signal(this.authState.isAuthenticated());
}
