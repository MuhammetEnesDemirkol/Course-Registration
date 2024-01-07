-- CREATE TABLE ogretmenBilgileri (
--   isim VARCHAR(255) NOT NULL,
--   soyisim VARCHAR(255) NOT NULL,
--   telefon VARCHAR(255) NOT NULL PRIMARY KEY
-- );

-- ALTER TABLE ogretmenBilgileri DROP CONSTRAINT ogretmenBilgileri_pkey

--//////////////////////////////////////////////////////////////////////////

-- CREATE TABLE ogrenciBilgileri (
--   tc_kimlik_numarasi VARCHAR(11) PRIMARY KEY,
--   isim VARCHAR(255) NOT NULL,
--   soyisim VARCHAR(255) NOT NULL,
--   dogum_yeri VARCHAR(255) NOT NULL,
--   dogum_tarihi DATE NOT NULL,
--   telefon_numarasi VARCHAR(255) NOT NULL
-- );
-- ALTER TABLE ogrenciBilgileri
--   ALTER COLUMN dogum_tarihi TYPE varchar(255) USING to_char(dogum_tarihi, 'DD-MM-YYYY');
-- ALTER TABLE ogrenciBilgileri
--   ADD COLUMN cinsiyet VARCHAR(255);
-- ALTER TABLE ogrenciBilgileri DROP CONSTRAINT ogrenciBilgileri_pkey

--//////////////////////////////////////////////////////////////////////////

-- CREATE TABLE alinanDersBilgileri (
--   tc_kimliknumarasi VARCHAR(11) PRIMARY KEY,
--   ders_kodu VARCHAR(255) NOT NULL,
--   vize_notu INTEGER,
--   final_notu INTEGER,
--   ortalama FLOAT,
--   CHECK (ortalama = (vize_notu * 0.4) + (final_notu * 0.6))
-- );

-- ALTER TABLE alinanDersBilgileri
-- ALTER COLUMN vize_notu SET DEFAULT 0;

-- ALTER TABLE alinanDersBilgileri
-- ALTER COLUMN final_notu SET DEFAULT 0;

-- ALTER TABLE alinanDersBilgileri DROP CONSTRAINT alinanDersBilgileri_pkey

--//////////////////////////////////////////////////////////////////////////--
