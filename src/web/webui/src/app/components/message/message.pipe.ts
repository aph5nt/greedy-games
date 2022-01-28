import { Pipe, PipeTransform } from '@angular/core';

export function isJsonString(str:string) {
    try {
        JSON.parse(str);
    } catch (e) {
        return false;
    }
    return true;
}

@Pipe({ name: 'messageFmt' })
export class MessageFmt implements PipeTransform {
    transform(value: any): string[] {
        if (isJsonString(value)) {
            return JSON.parse(value).errors;
        } else {
            return [value];
        }
    }
}


