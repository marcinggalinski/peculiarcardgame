import type { InjectionKey } from "vue";
import type DeckManagementApiService from "@/services/deck-management/apiService";
import type UsersApiService from "@/services/users/apiService";

export const DeckManagementApiServiceKey = Symbol("DeckManagementApiService") as InjectionKey<DeckManagementApiService>;

export const UsersApiServiceKey = Symbol("UsersApiService") as InjectionKey<UsersApiService>;
