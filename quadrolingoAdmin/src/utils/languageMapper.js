// Convert backend data to frontend format
export const fromBackend = (backendWord) => {
  return {
    id: backendWord.Id,
    lang: backendWord.WORD_LANG,
    base: backendWord.WORD_BASE,
    translations: JSON.parse(backendWord.WORD_TRANSLATION), // Convert translation string back to object
  };
};

// Convert frontend data to backend format
export const toBackend = (frontendWord) => {
  return {
    Id: frontendWord.id,
    WORD_LANG: frontendWord.lang,
    WORD_BASE: frontendWord.base,
    WORD_TRANSLATION: JSON.stringify(frontendWord.translations), // Convert translations object to string
  };
};