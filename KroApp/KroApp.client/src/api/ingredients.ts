import { post } from "@/api/api";
import type { AxiosResponse } from "axios";
import {
  IngredientSearchRequestDTO,
  IngredientSearchResultDTO,
} from "@/api/DTOs/IngredientDTOs";

async function searchIngredients(
  query: string,
): Promise<IngredientSearchResultDTO[]> {
  const body: IngredientSearchRequestDTO = {
    query: query,
    dataType: ["Foundation", "SR Legacy", "Survey (FNDDS)", "Branded"],
    pageSize: 25,
    pageNumber: 1,
    sortBy: "dataType.keyword",
    sortOrder: "asc",
    brandOwner: "",
  };
  const response: AxiosResponse = await post<AxiosResponse>(
    "/Ingredients/search",
    body,
  );
  return response.data as IngredientSearchResultDTO[];
}

export { searchIngredients as listIngredients };
