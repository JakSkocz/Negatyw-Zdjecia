.code
MyProc1 proc
pmovzxbd xmm0, [rcx]		;przepisanie na rejestr xmm0 tablice pikseli
pmovzxbd xmm1, [rdx]		;przepisanie na rejestr xmm1 tablicy z wartosciami 255
subps xmm1, xmm0			;odejmowanie wartosci na pierwszym kanale
movd eax, xmm1				;przepisanie na rejestr eax wartosci odjêtej
mov [rcx], al				
pmovzxbd xmm0, [rcx+1]		;przepisanie na rejestr xmm0 tablice pikseli
pmovzxbd xmm1, [rdx+1]		;przepisanie na rejestr xmm1 tablicy z wartosciami 255
subps xmm1, xmm0			;odejmowanie wartosci na drugim kanale
movd eax, xmm1				;przepisanie na rejestr eax wartosci odjêtej
mov [rcx+1], al				
pmovzxbd xmm0, [rcx+2]		;przepisanie na rejestr xmm0 tablice pikseli
pmovzxbd xmm1, [rdx+2]		;przepisanie na rejestr xmm1 tablicy z wartosciami 255
subps xmm1, xmm0			;odejmowanie wartosci na trzecim kanale
movd eax, xmm1				;przepisanie na rejestr eax wartosci odjêtej
mov [rcx+2], al				
ret
MyProc1 endp
end