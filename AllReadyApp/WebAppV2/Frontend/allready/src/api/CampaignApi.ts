import { Campaign } from '@/models/Campaign';

class CampaignApi {
    public async getCampaigns(): Promise<Campaign[]> {
        const response = await fetch(`${process.env.VUE_APP_API_URL}/campaigns`);
        if (response.ok) {
            return await response.json();
        }

        throw new Error(`Error response code ${response.status} - ${response.statusText}`);
    }

}

export let campaignApi = new CampaignApi();
