// Convert backend data to frontend format
export const fromBackend = (backendWord) => {
  return {
    id: backendWord.id,
    lang: backendWord.worD_LANG,
    base: backendWord.worD_BASE,
    translations: JSON.parse(backendWord.worD_TRANSLATION), // Convert translation string back to object
  };
};

// Convert frontend data to backend format
export const toBackend = (frontendWord) => {
  return {
    id: frontendWord.id,
    worD_LANG: frontendWord.lang,
    worD_BASE: frontendWord.base,
    worD_TRANSLATION: JSON.stringify(frontendWord.translations), // Convert translations object to string
  };
};