export interface Campaign {
    id: number;
    name: string;
    shortDescription: string;
    hidden: boolean;
    startDateTime: Date;
    endDateTime: Date;
    timezone: string;
}
