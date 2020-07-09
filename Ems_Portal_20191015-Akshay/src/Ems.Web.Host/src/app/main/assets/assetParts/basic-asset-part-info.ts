import * as moment from "moment";

export interface IBasicAssetPartInfo {
    assetId: number;
    id: number;
    name: string;
    description: string;
    serialNumber: string;
    installDate: moment.Moment;
    installed: string;
    assetPartType: string;
    assetPartStatus: string;
    assetReference: string;
    itemType: string;
    code: string;
}
