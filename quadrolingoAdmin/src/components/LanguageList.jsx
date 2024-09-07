// LanguagesList component that takes initialLanguages as a prop
const LanguagesList = ({ initialLanguages }) => {
  return (
    <div>
      <h2>Languages</h2>
      <ul>
        {initialLanguages.map((language, index) => (
          <li key={index}>
            <strong>{language.code}</strong>: {language.name}
          </li>
        ))}
      </ul>
    </div>
  );
};

export default LanguagesList;
