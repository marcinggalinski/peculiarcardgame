import type { InjectionKey } from "vue";
import type DeckManagementApiService from "@/services/deck-management/apiService";
import type UsersApiService from "@/services/users/apiService";
import type UsersService from "@/services/users/UsersService";

export const DeckManagementApiServiceKey = Symbol("DeckManagementApiService") as InjectionKey<DeckManagementApiService>;
export const UsersApiServiceKey = Symbol("UsersApiService") as InjectionKey<UsersApiService>;
export const UsersServiceKey = Symbol("UsersService") as InjectionKey<UsersService>;
