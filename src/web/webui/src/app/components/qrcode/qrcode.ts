import {
    NgModule,
    Component,
    Input,
    ElementRef,
    OnChanges,
    SimpleChanges
} from '@angular/core';

//import QRious from 'qrious';

//import * as jQuery from 'jquery';
declare var QRious: any;

@Component({
    moduleId: 'module.id',
    selector: 'app-qrcode',
    template: ``
})
export class QRCodeComponent implements OnChanges {

    @Input() background = 'white';
    @Input() backgroundAlpha = 1.0;
    @Input() foreground = 'black';
    @Input() foregroundAlpha = 1.0;
    @Input() level = 'L';
    @Input() mime = 'image/png';
    @Input() padding: number = null;
    @Input() size = 100;
    @Input() value = '';
    @Input() canvas = false;

    ngOnChanges(changes: SimpleChanges): void {
        if ('background' in changes ||
            'backgroundAlpha' in changes ||
            'foreground' in changes ||
            'foregroundAlpha' in changes ||
            'level' in changes ||
            'mime' in changes ||
            'padding' in changes ||
            'size' in changes ||
            'value' in changes ||
            'canvas' in changes) {
            this.generate();
        }
    }

    constructor(private elementRef: ElementRef) {
    }

    generate() {
        try {
            const el: HTMLElement = this.elementRef.nativeElement;
            el.innerHTML = '';

            var qr = new QRious();
            qr.mime = "image/png";
            qr.padding = this.padding;
            qr.size = this.size;
            qr.value = this.value;

            if (this.canvas) {
                el.appendChild(qr.canvas);
            } else {
                el.appendChild(qr.image);
            }
        } catch (e) {
            console.error(`Could not generate QR Code: ${e.message}`);
        }
    }
}

@NgModule({
    exports: [QRCodeComponent],
    declarations: [QRCodeComponent]
})
export class QRCodeModule {
}