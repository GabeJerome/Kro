export interface IngredientSearchRequestDTO {
  query: string;
  dataType: ("Branded" | "Foundation" | "Survey (FNDDS)" | "SR Legacy")[];
  pageSize?: number;
  pageNumber?: number;
  sortBy?:
    | "dataType.keyword"
    | "lowercaseDescription.keyword"
    | "fdcid"
    | "publishedDate";
  sortOrder?: "asc" | "desc";
  brandOwner?: string;
  tradeChannel?: string[];
  startDate?: string;
  endDate?: string;
}

export interface IngredientSearchResultDTO {
  FdcId: number;
  Name: string;
  Brand: string;
  PackageWeight: string;
  ServingSize: string;
  Ingredients: string[];
  highlightFields: string;
}
