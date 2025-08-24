import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-confirmation-dialog',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="dialog-overlay" [class.active]="isOpen" (click)="onOverlayClick($event)">
      <div class="dialog-container" role="dialog" aria-modal="true">
        <div class="dialog-header">
          <h3 class="dialog-title">{{ title }}</h3>
          <button class="dialog-close" (click)="cancel.emit()" aria-label="Close dialog">
            <i class="pi pi-times"></i>
          </button>
        </div>
        
        <div class="dialog-body">
          <p class="dialog-message">{{ message }}</p>
        </div>
        
        <div class="dialog-footer">
          <button 
            class="btn-secondary" 
            (click)="cancel.emit()" 
            [attr.aria-label]="cancelText"
          >
            {{ cancelText }}
          </button>
          <button 
            class="btn-danger" 
            (click)="confirm.emit()" 
            [attr.aria-label]="confirmText"
          >
            {{ confirmText }}
          </button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .dialog-overlay {
      position: fixed;
      top: 0;
      left: 0;
      right: 0;
      bottom: 0;
      background-color: rgba(0, 0, 0, 0.5);
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 50;
      opacity: 0;
      visibility: hidden;
      transition: opacity 0.2s ease, visibility 0.2s ease;
    }
    
    .dialog-overlay.active {
      opacity: 1;
      visibility: visible;
    }
    
    .dialog-container {
      background-color: var(--background);
      border-radius: 0.5rem;
      width: 100%;
      max-width: 28rem;
      box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04);
      transform: translateY(0);
      transition: transform 0.3s ease;
    }
    
    .dialog-overlay:not(.active) .dialog-container {
      transform: translateY(1rem);
    }
    
    .dialog-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 1rem 1.5rem;
      border-bottom: 1px solid var(--border);
    }
    
    .dialog-title {
      font-size: 1.25rem;
      font-weight: 600;
      color: var(--foreground);
      margin: 0;
    }
    
    .dialog-close {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 2rem;
      height: 2rem;
      border-radius: 0.375rem;
      border: none;
      background-color: transparent;
      color: var(--muted-foreground);
      cursor: pointer;
      transition: all 0.2s ease;
    }
    
    .dialog-close:hover {
      background-color: var(--muted);
      color: var(--foreground);
    }
    
    .dialog-body {
      padding: 1.5rem;
    }
    
    .dialog-message {
      color: var(--foreground);
      margin: 0;
      line-height: 1.5;
    }
    
    .dialog-footer {
      display: flex;
      justify-content: flex-end;
      gap: 0.75rem;
      padding: 1rem 1.5rem;
      border-top: 1px solid var(--border);
    }
    
    .btn-secondary {
      display: inline-flex;
      align-items: center;
      justify-content: center;
      padding: 0.5rem 1rem;
      font-size: 0.875rem;
      font-weight: 500;
      border-radius: 0.375rem;
      border: 1px solid var(--border);
      background-color: var(--background);
      color: var(--foreground);
      cursor: pointer;
      transition: all 0.2s ease;
    }
    
    .btn-secondary:hover {
      background-color: var(--muted);
    }
    
    .btn-danger {
      display: inline-flex;
      align-items: center;
      justify-content: center;
      padding: 0.5rem 1rem;
      font-size: 0.875rem;
      font-weight: 500;
      border-radius: 0.375rem;
      border: none;
      background-color: var(--destructive);
      color: white;
      cursor: pointer;
      transition: all 0.2s ease;
    }
    
    .btn-danger:hover {
      background-color: var(--destructive-hover, var(--destructive));
      filter: brightness(0.9);
    }
  `]
})
export class ConfirmationDialog {
  @Input() isOpen = false;
  @Input() title = 'Confirm Action';
  @Input() message = 'Are you sure you want to perform this action?';
  @Input() confirmText = 'Confirm';
  @Input() cancelText = 'Cancel';
  
  @Output() confirm = new EventEmitter<void>();
  @Output() cancel = new EventEmitter<void>();
  
  onOverlayClick(event: MouseEvent) {
    // Only close if the overlay itself is clicked, not the dialog content
    if ((event.target as HTMLElement).classList.contains('dialog-overlay')) {
      this.cancel.emit();
    }
  }
}
