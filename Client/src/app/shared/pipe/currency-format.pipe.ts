import { Pipe, type PipeTransform } from '@angular/core';

@Pipe({
  name: 'appCurrencyFormat',
})
export class CurrencyFormatPipe implements PipeTransform {

 transform(amount: number | undefined): string {
    if (typeof amount !== 'number') return '';
    return new Intl.NumberFormat('en-ZA', {
      style: 'currency',
      currency: 'ZAR'
    }).format(amount);
  }
}
