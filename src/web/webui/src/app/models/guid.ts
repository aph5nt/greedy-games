export class Guid {

    public static newGuid(): string {
        return this.generateGuid();
    }

    private static generateGuid(): string {
        return this.generatePart() + this.generatePart() + '-' + this.generatePart() + '-' + this.generatePart() + '-' +
            this.generatePart() + '-' + this.generatePart() + this.generatePart() + this.generatePart();
    }

    private static generatePart(): string {
        return Math.floor((1 + Math.random()) * 0x10000)
            .toString(16)
            .substring(1);
    }
}