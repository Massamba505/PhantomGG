import { Component } from '@angular/core';
import { MessageService } from 'primeng/api';
import { primengModules } from '../../primeng/primeng-config';

@Component({
  selector: 'app-toast',
  standalone: true,
  imports: [...primengModules],
  template: ` <p-toast position="top-right"></p-toast> `,
  styleUrl: './toast.component.css',
})
export class ToastComponent {}
