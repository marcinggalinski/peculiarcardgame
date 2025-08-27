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
  async addCard(deckId: number, text: string, cardType: CardType): Promise<GetCardResponse> {
    const request: AddCardRequest = {
      cardType,
      text,
    };
    const response = await this.instance.post<GetCardResponse>(`decks/${deckId}/cards`, request);

    switch (response.status) {
      case 201:
        return response.data;
      default:
        throw new ApiError(response.status, response.statusText);
    }
  }

  async addDeck(name: string, description?: string): Promise<GetDeckResponse> {
    const request: AddDeckRequest = {
      name,
      description,
    };
    const response = await this.instance.post<GetDeckResponse>("decks", request);

    switch (response.status) {
      case 201:
        return response.data;
      default:
        throw new ApiError(response.status, response.statusText);
    }
  }

  async deleteCard(cardId: number): Promise<void> {
    const response = await this.instance.delete(`cards/${cardId}`);

    switch (response.status) {
      case 200:
        return;
      default:
        throw new ApiError(response.status, response.statusText);
    }
  }

  async deleteDeck(deckId: number): Promise<void> {
    const response = await this.instance.delete(`decks/${deckId}`);

    switch (response.status) {
      case 200:
        return;
      default:
        throw new ApiError(response.status, response.statusText);
    }
  }

  async getCard(cardId: number): Promise<GetCardResponse> {
    const response = await this.instance.get<GetCardResponse>(`decks/${cardId}`);

    switch (response.status) {
      case 200:
        return response.data;
      default:
        throw new ApiError(response.status, response.statusText);
    }
  }

  async getCards(deckId: number, query?: string): Promise<GetCardResponse[]> {
    const response = await this.instance.get<GetCardResponse[]>(
      query ? `decks/${deckId}/cards?query=${query}` : `decks/${deckId}/cards`
    );

    switch (response.status) {
      case 200:
        return response.data;
      default:
        throw new ApiError(response.status, response.statusText);
    }
  }

  async getDeck(deckId: number): Promise<GetDeckResponse> {
    const response = await this.instance.get<GetDeckResponse>(`decks/${deckId}`);

    switch (response.status) {
      case 200:
        return response.data;
      default:
        throw new ApiError(response.status, response.statusText);
    }
  }

  async getDecks(query?: string): Promise<GetDeckResponse[]> {
    const response = await this.instance.get<GetDeckResponse[]>(query ? `decks?query=${query}` : "decks");

    switch (response.status) {
      case 200:
        return response.data;
      default:
        throw new ApiError(response.status, response.statusText);
    }
  }

  async updateCard(cardId: number, textUpdate: string): Promise<GetCardResponse> {
    const request: UpdateCardRequest = {
      textUpdate,
    };
    const response = await this.instance.patch<GetCardResponse>(`cards/${cardId}`, request);

    switch (response.status) {
      case 200:
        return response.data;
      default:
        throw new ApiError(response.status, response.statusText);
    }
  }

  async updateDeck(deckId: number, nameUpdate?: string, descriptionUpdate?: string): Promise<GetDeckResponse> {
    const request: UpdateDeckRequest = {
      descriptionUpdate,
      nameUpdate,
    };
    const response = await this.instance.patch<GetDeckResponse>(`decks/${deckId}`, request);

    switch (response.status) {
      case 200:
        return response.data;
      default:
        throw new ApiError(response.status, response.statusText);
    }
  }
}
