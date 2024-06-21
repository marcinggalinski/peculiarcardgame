import type {
  GetCardResponse,
  AddCardRequest,
  AddDeckRequest,
  CardType,
  GetDeckResponse,
  UpdateCardRequest,
  UpdateDeckRequest,
} from "@/models/deck-management/api";
import ApiError from "@/models/apiError";
import ApiServiceBase from "@/services/apiServiceBase";

export default class ApiService extends ApiServiceBase {
  async addCard(deckId: number, text: string, cardType: CardType) {
    const request: AddCardRequest = {
      cardType,
      text,
    };
    const response = await this.instance.post<GetCardResponse>(`decks/${deckId}/cards`, request);
    if (response.status !== 201) {
      throw new ApiError(response.status, response.statusText);
    }
    return response.data;
  }

  async addDeck(name: string, description?: string) {
    const request: AddDeckRequest = {
      name,
      description,
    };
    const response = await this.instance.post<GetDeckResponse>("decks", request);
    if (response.status !== 201) {
      throw new ApiError(response.status, response.statusText);
    }
    return response.data;
  }

  async deleteCard(cardId: number) {
    const response = await this.instance.delete(`cards/${cardId}`);
    if (response.status !== 200) {
      throw new ApiError(response.status, response.statusText);
    }
  }

  async deleteDeck(deckId: number) {
    const response = await this.instance.delete(`decks/${deckId}`);
    if (response.status !== 200) {
      throw new ApiError(response.status, response.statusText);
    }
  }

  async getCard(cardId: number) {
    const response = await this.instance.get<GetCardResponse>(`decks/${cardId}`);
    if (response.status !== 200) {
      throw new ApiError(response.status, response.statusText);
    }
    return response.data;
  }

  async getCards(deckId: number, query?: string) {
    const response = await this.instance.get<GetCardResponse[]>(
      query ? `decks/${deckId}/cards?query=${query}` : `decks/${deckId}/cards`
    );
    if (response.status !== 200) {
      throw new ApiError(response.status, response.statusText);
    }
    return response.data;
  }

  async getDeck(deckId: number) {
    const response = await this.instance.get<GetDeckResponse>(`decks/${deckId}`);
    if (response.status !== 200) {
      throw new ApiError(response.status, response.statusText);
    }
    return response.data;
  }

  async getDecks(query?: string) {
    const response = await this.instance.get<GetDeckResponse[]>(query ? `decks?query=${query}` : "decks");
    if (response.status !== 200) {
      throw new ApiError(response.status, response.statusText);
    }
    return response.data;
  }

  async updateCard(cardId: number, textUpdate?: string) {
    const request: UpdateCardRequest = {
      textUpdate,
    };
    const response = await this.instance.patch<GetCardResponse>(`cards/${cardId}`, request);
    if (response.status !== 200) {
      throw new ApiError(response.status, response.statusText);
    }
    return response.data;
  }

  async updateDeck(deckId: number, descriptionUpdate?: string, nameUpdate?: string) {
    const request: UpdateDeckRequest = {
      descriptionUpdate,
      nameUpdate,
    };
    const response = await this.instance.patch<GetDeckResponse>(`decks/${deckId}`, request);
    if (response.status !== 200) {
      throw new ApiError(response.status, response.statusText);
    }
    return response.data;
  }
}
