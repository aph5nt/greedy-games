import { NgModule } from '@angular/core';
import { CentBit, Embit, Youbit, CentBitPrefix, Currency, CurrencyPrefix, CurrencyInput, YoubitPrefix, EmbitPrefix } from './currency.converter.pipe';

export const pipes = [
    CentBit,
    Embit,
    Youbit,
    CentBitPrefix,
    EmbitPrefix,
    YoubitPrefix,
    CurrencyPrefix,
    CurrencyInput,
    Currency
];

@NgModule({
  declarations: pipes,
  exports: pipes
})
export class CurrencyModule { }

