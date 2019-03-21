import Vue from 'vue';
import Vuex, { StoreOptions, ActionTree, MutationPayload, Store } from 'vuex';
import { RootState } from './state';
import { campaigns } from './modules/campaigns';

Vue.use(Vuex);


const store: StoreOptions<RootState> = {
    strict: true,
    state: {
        showError: false,
        errorMessage: ''
    },
    actions: {
        showErrorMessage({ commit, state }, errorMessage) {
            commit('showErrorMessage', errorMessage);
        },
        hideError({ commit }) {
            commit('hideError');
        }

    },
    mutations: {
        showErrorMessage(state, errorMessage) {
            state.showError = true;
            state.errorMessage = errorMessage || 'Oops. Something went wrong.';
        },
        hideError(state) {
            state.showError = false;
            state.errorMessage = '';
        }
    },
    modules: {
        campaigns
    }
};

export default new Vuex.Store<RootState>(store);
