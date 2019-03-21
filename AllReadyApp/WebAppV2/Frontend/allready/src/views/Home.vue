<template>
  <div class="home">
    <v-container fluid grid-list-md>
      <v-layout row wrap>
        <v-flex v-for="campaign in upcomingCampaigns" :key="campaign.id">
          <v-card>
            <v-img :src="'https://picsum.photos/200/400/?random' + campaign.id" height="200px"></v-img>
            <v-card-title primary-title>
              <div>
                <div class="headline">{{campaign.name}}</div>
                <span class="grey--text">{{campaign.shortDescription}}</span>
              </div>
            </v-card-title>
            <v-card-actions>
              <v-btn flat color="primary" :to="'/campaign/' + campaign.id">More Info</v-btn>
              <v-spacer></v-spacer>
            </v-card-actions>
          </v-card>
        </v-flex>
      </v-layout>
    </v-container>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';
import { State, Action, Getter } from 'vuex-class';
import { CampaignState } from '@/store/modules/state';
import { Campaign } from '@/models/Campaign';

@Component({
  components: {}
})
export default class Home extends Vue {
  @State('campaigns') private campaignState!: CampaignState;

  @Action('getUpcomingCampaigns', { namespace: 'campaigns' })
  private getUpcomingCampaigns!: () => void;

  protected mounted() {
    this.getUpcomingCampaigns();
  }

  private get upcomingCampaigns(): Campaign[] {
    return this.campaignState.campaigns;
  }
}
</script>
