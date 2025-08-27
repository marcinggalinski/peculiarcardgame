import type { InjectionKey } from "vue";
import type DeckManagementApiService from "@/services/deck-management/apiService";
import type UsersService from "@/services/users/usersService";

export const DeckManagementApiServiceKey = Symbol("DeckManagementApiService") as InjectionKey<DeckManagementApiService>;
export const UsersServiceKey = Symbol("UsersService") as InjectionKey<UsersService>;
