import { campaignApi } from '@/api/CampaignApi';
import { Module, ActionTree, MutationTree, GetterTree, Mutation, MutationPayload, Store } from 'vuex';
import { CampaignState } from './state';
import { RootState } from '../state';
import { Campaign } from '@/models/Campaign';

// initial state
const campaignState: CampaignState = {
    campaigns: []
};

// getters
const getters: GetterTree<CampaignState, RootState> = {

};

const actions: ActionTree<CampaignState, RootState> = {
    async getUpcomingCampaigns({ commit, dispatch }): Promise<any> {
        try {
            const upcomingCampaigns = await campaignApi.getCampaigns();
            commit('setCampaigns', upcomingCampaigns);
        } catch (e) {
            dispatch('showErrorMessage', e.message, { root: true });
        }
    }
};

const mutations: MutationTree<CampaignState> = {
    setCampaigns(s, campaings: Campaign[]) {
        s.campaigns = campaings;
    }
};

export const campaigns: Module<CampaignState, RootState> = {
    namespaced: true,
    state: campaignState,
    getters,
    actions,
    mutations
};
