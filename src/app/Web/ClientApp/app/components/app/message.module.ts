import { NgModule } from '@angular/core';
import { MessageFmt } from "./message.pipe";

@NgModule({
    declarations: [MessageFmt],
    exports: [MessageFmt]
})
export class MessageModule { }