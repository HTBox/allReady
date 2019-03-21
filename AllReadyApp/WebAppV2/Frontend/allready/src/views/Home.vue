<template>
  <div class="home">
      <h1>Welcome to AllReady</h1>
      <ul>
          <li v-for="c in upcomingCampaigns" :key="c.id">{{c.name}}</li>
          </ul>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';
import { State, Action, Getter } from 'vuex-class';
import {CampaignState} from '@/store/modules/state';
import { Campaign } from '@/models/Campaign';

@Component({
  components: {
  },
})
export default class Home extends Vue {
    @State('campaigns') private campaignState!: CampaignState;

    @Action('getUpcomingCampaigns', { namespace: 'campaigns' })
   private getUpcomingCampaigns!: () => void;

    protected mounted() {
        this.getUpcomingCampaigns();
    }

    private get upcomingCampaigns() : Campaign[];
}
</script>
