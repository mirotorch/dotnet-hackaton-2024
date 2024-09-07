// Convert backend data to frontend format
export const fromBackend = (backendLang) => {
  return {
    code: backendLang.lanG_CODE,
    name: backendLang.lanG_ENG_NAME,
  };
};

// Convert frontend data to backend format
export const toBackend = (frontendLang) => {
  return {
    lanG_CODE: frontendLang.code,
    lanG_ENG_NAME: frontendLang.name,
  };
};