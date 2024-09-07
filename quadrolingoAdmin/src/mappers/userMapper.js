// Convert backend data to frontend format
export const fromBackend = (backendLang) => {
  return {
    base_lang: backendLang.basE_LANG,
    chat_id: backendLang.chaT_ID,
    first_name: backendLang.firsT_NAME,
    id: backendLang.id,
    last_name: backendLang.lasT_NAME,
    study_lang: backendLang.studY_LANG,
    telegram_id: backendLang.telegraM_ID,
    username: backendLang.username,
  };
};