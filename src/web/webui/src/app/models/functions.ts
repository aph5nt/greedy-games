import * as moment from 'moment';
import { CentBit } from '../components/currency/currency.converter.pipe';
import { Injectable } from '@angular/core';

@Injectable()
export class Helpers {

  toLocalTime(date: Date) {
    const localTime = moment.utc(date).toDate();
    return moment(localTime).format('YYYY-MM-DD HH:mm:ss');
  }

  toLocalDate(date: Date) {
    const localTime = moment.utc(date).toDate();
    return moment(localTime).format('YYYY-MM-DD');
  }
  
  showProfit(item: any) {
    if (item.loss > 0) {
      return new CentBit().transform(-item.loss);
    } else {
      return " " + new CentBit().transform(item.win - item.bet);
    }
  }
 
}

