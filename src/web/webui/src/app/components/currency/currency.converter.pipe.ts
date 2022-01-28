import { Pipe, PipeTransform } from '@angular/core';

@Pipe({name: 'currency'})
export class Currency implements PipeTransform {
  transform(value: number): string {
      return new CentBit().transform(value);
  }
}

@Pipe({name: 'currencyPrefix'})
export class CurrencyPrefix implements PipeTransform {
  transform(value: string): string {
      return new CentBitPrefix().transform(value);
  }
}

@Pipe({name: 'toSatoshi'})
export class CurrencyInput implements PipeTransform {
  transform(value: number): number {
      return value;
  }
}

@Pipe({name: 'centbit'})
export class CentBit implements PipeTransform {
  transform(value: number): string {
      return (value / 1000000).toFixed(2);
  }
}

@Pipe({name: 'centbitPrefix'})
export class CentBitPrefix implements PipeTransform {
  transform(value: string): string {
       return '&cent;' + value;
  }
}

@Pipe({name: 'embitPrefix'})
export class EmbitPrefix implements PipeTransform {
  transform(value: string): string {
       return 'm' + value;
  }
}

@Pipe({name: 'embit'})
export class Embit implements PipeTransform {
  transform(value: number): string {
      return (value / 100000).toFixed(3);
  }
}

@Pipe({name: 'youbit'})
export class Youbit implements PipeTransform {
  transform(value: number): string {
      return (value / 100).toFixed(6);
  }
}

@Pipe({name: 'youbitPrefix'})
export class YoubitPrefix implements PipeTransform {
  transform(value: string): string {
       return '&micro;' + value;
  }
}

